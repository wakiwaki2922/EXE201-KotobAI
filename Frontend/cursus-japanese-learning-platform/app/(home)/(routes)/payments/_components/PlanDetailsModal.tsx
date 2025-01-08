import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from "@/components/ui/dialog";
import {
  PayPalScriptProvider,
  PayPalButtons,
  ReactPayPalScriptOptions,
} from "@paypal/react-paypal-js";
import { PackagePlan } from "@/types/PackagePlan";
import { formatCurrency } from "@/lib/utils";
import { useEffect, useState } from "react";
import { DollarSign, Calendar, Clock, Package } from "lucide-react"; // Import icon
import { createSubscriptionPayment } from "@/actions/payment-subscription";
import { useToast } from "@/hooks/use-toast";

interface PlanDetailsModalProps {
  selectedPlan: PackagePlan | null;
  onClose: () => void;
}

const PlanDetailsModal: React.FC<PlanDetailsModalProps> = ({
  selectedPlan,
  onClose,
}) => {
  const clientId = process.env.PAYPAL_CLIENT_ID;
  const { toast } = useToast();
  const [startDate, setStartDate] = useState<Date | null>(null);
  const [endDate, setEndDate] = useState<Date | null>(null);

  useEffect(() => {
    if (selectedPlan) {
      const currentDate = new Date();
      const expirationDate = new Date(currentDate);
      expirationDate.setMonth(currentDate.getMonth() + selectedPlan.period);
      setStartDate(currentDate);
      setEndDate(expirationDate);
    }
  }, [selectedPlan]);

  if (!selectedPlan) return null;

  const onClickProceedPayment = async () => {
    const payment_approval_url = await createSubscriptionPayment(selectedPlan);
    window.location.href = payment_approval_url.data;
  };

  const onError = (err: any) => {
    console.error("Error with payment: ", err);
    toast({
      variant: "destructive",
      title: "Uh oh! Something went wrong.",
      description: "An error occurred while processing your payment.",
      duration: 5000,
    });
  };

  const onCancel = () => {
    toast({
      variant: "default",
      title: "Payment cancelled",
      description: "You have cancelled the payment process.",
      duration: 5000,
      className: "bg-yellow-500",
    });
  };

  return (
    <Dialog open={!!selectedPlan} onOpenChange={onClose}>
      <DialogContent className="max-w-md p-6 bg-sky-50 rounded-lg shadow-lg">
        <DialogHeader className="text-center">
          <DialogTitle className="text-2xl font-bold">
            {selectedPlan?.planName} Subscription
          </DialogTitle>
          <DialogDescription className="mt-1 text-sm text-gray-400">
            Unlock exclusive benefits with the {selectedPlan?.planName}.
          </DialogDescription>
        </DialogHeader>

        <div className="my-6 border-t border-gray-700"></div>

        <div className="space-y-4 text-sm leading-relaxed">
          <div className="flex items-center justify-between">
            <span className="font-semibold flex items-center gap-x-2">
              <Package size={16} /> Plan Name
            </span>
            <span>{selectedPlan?.planName}</span>
          </div>
          <div className="flex items-center justify-between">
            <span className="font-semibold flex items-center gap-x-2">
              <Package size={16} /> Plan Type
            </span>
            <span>{selectedPlan?.planType}</span>
          </div>
          <div className="flex items-center justify-between">
            <span className="font-semibold flex items-center gap-x-2">
              <Clock size={16} /> Duration
            </span>
            <span>{selectedPlan?.period} months</span>
          </div>
          <div className="flex items-center justify-between">
            <span className="font-semibold flex items-center gap-x-2">
              <Calendar size={16} /> Start Date
            </span>
            <span>{startDate?.toLocaleDateString()}</span>
          </div>
          <div className="flex items-center justify-between">
            <span className="font-semibold flex items-center gap-x-2">
              <Calendar size={16} /> End Date
            </span>
            <span>{endDate?.toLocaleDateString()}</span>
          </div>
          <div className="flex items-center justify-between">
            <span className="font-semibold flex items-center gap-x-2">
              <DollarSign size={16} /> Price
            </span>
            <span className="text-lg font-bold">
              {selectedPlan ? formatCurrency(Number(selectedPlan.price)) : ""}
            </span>
          </div>
        </div>

        <DialogFooter className="flex justify-end mt-8 space-x-2">
          <Button
            variant="outline"
            onClick={onClose}
            className="border-gray-500 hover:bg-slate-300"
          >
            Close
          </Button>
          <PayPalScriptProvider
            options={{ clientId: clientId } as ReactPayPalScriptOptions}
          >
            <PayPalButtons
              style={{
                layout: "horizontal",
                color: "blue",
                shape: "rect",
                height: 40,
                tagline: false,
                label: "subscribe",
                disableMaxWidth: true,
              }}
              onClick={onClickProceedPayment}
              onError={onError}
              onCancel={() => {
                console.log("Payment cancelled");
                onCancel();
              }}
              disabled={false}
            />
          </PayPalScriptProvider>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

export default PlanDetailsModal;
